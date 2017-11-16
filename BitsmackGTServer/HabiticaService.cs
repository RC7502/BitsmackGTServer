using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using BSFullCalendar.Models;
using HabitRPG.NET;
using HabitRPG.NET.Models;
using Toodledo.Model.API;
using Task = HabitRPG.NET.Models.Task;

namespace BitsmackGTServer
{
    internal class HabiticaService : BaseService
    {
        public HabiticaService() 
        {
            todoModel = new ToodledoModel();
            todoList = new List<Toodledo.Model.Task>();
            habitClient = new HabitRPGClient();
            habitTasks = new List<HabitRPG.NET.Models.Task>();
        }

        public ToodledoModel todoModel { get; set; }
        public List<Toodledo.Model.Task> todoList { get; set; }
        public HabitRPGClient habitClient { get; set; }
        public List<HabitRPG.NET.Models.Task> habitTasks { get; set; }

        public bool Update(int interval)
        {
            //Logger.LogDebug("Starting Habitica.Update");

            try
            {
                todoList =  GetNewTasks(interval);
                MergeTasks(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return false;
            }

            //Logger.LogDebug(string.Format("Completed Habitica.Update"));
            return true;
        }

        private void MergeTasks(bool FirstPass)
        {
            foreach (var todo in todoList)
            {
                try
                {
                    Task habitTask;
                    if (FirstPass)
                    {
                        habitTask = habitTasks.FirstOrDefault(x => x.Alias == todo.Id.ToString()) ??
                                    habitClient.GetTask(todo.Id.ToString());
                    }
                    else
                    {
                        habitTask = habitClient.GetTask(todo.Id.ToString());
                    }
                   
                    var habitid = "";
                    bool? habitDone = false;
                    if (habitTask == null)
                    {
                        var newTask = new HabitRPG.NET.Models.Task()
                        {
                            Type = "todo",
                            Text = todo.Name,
                            Alias = todo.Id.ToString(),                            
                        };
                        //if (!todo.Due.Year.Equals(1))
                        //    newTask.Date = todo.Due.ToShortDateString();
                        habitid = habitClient.AddTask(newTask);
                        Logger.LogDebug("Added Task: " + newTask.Text);
                    }
                    else
                    {
                        habitid = habitTask.Id.ToString();
                        habitDone = habitTask.Completed;
                    }
                    if (!todo.Completed.Year.Equals(1) && !habitDone.GetValueOrDefault())
                    {
                        habitClient.ScoreTask(habitid, "up");
                        Logger.LogDebug("Completed Task: " + todo.Name);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(string.Format("MergeTasks failed on item {0}. {1}", todo.Name, e.Message));
                }
                

            }
        }

        private List<HabitRPG.NET.Models.Task> GetTasks()
        {
            return habitClient.GetTasks().Where(x=>x.Type == "todo").ToList();
        }

        private List<Toodledo.Model.Task> GetOpenAndRecentTasks()
        {
            var returnVal = new List<Toodledo.Model.Task>();
            var query = new TaskQuery()
            {
                NotCompleted = true,
            };
            returnVal = todoModel.Tasks.GetTasks(query).Tasks.ToList();
            query.NotCompleted = false;
            query.CompletedAfter = DateTime.Now.AddDays(-1);
            returnVal.AddRange(todoModel.Tasks.GetTasks(query).Tasks.ToList());
            return returnVal;
        }

        private List<Toodledo.Model.Task> GetNewTasks(int interval)
        {
            var query = new TaskQuery()
            {
                ModifiedAfter = DateTime.Now.AddMinutes(-1 * interval)
            };
            return todoModel.Tasks.GetTasks(query).Tasks.ToList();

        }


        public bool FirstRun()
        {
            Logger.LogDebug("Starting Habitica.FirstRun");

            try
            {
                todoList = GetOpenAndRecentTasks();
                habitTasks = GetTasks();
                DeleteTasks();
                MergeTasks(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return false;
            }

            Logger.LogDebug(string.Format("Completed Habitica.FirstRun"));
            return true;
        }

        private void DeleteTasks()
        {
            foreach (var habitTask in habitTasks)
            {
                if (string.IsNullOrEmpty(habitTask.Alias))
                {
                    habitClient.DeleteTask(habitTask.Id);
                }
                //else make sure toodledo item hasn't been deleted
                else
                {
                    int todoid;
                    if (int.TryParse(habitTask.Alias, out todoid))
                    {
                        var query = new TaskQuery()
                        {
                            Id = Convert.ToInt32(habitTask.Alias)
                        };
                        var todo = todoModel.Tasks.GetTasks(query).Tasks.FirstOrDefault();
                        if (todo == null)
                        {
                            habitClient.DeleteTask(habitTask.Id);
                        }
                    }
                    else
                    {
                        habitClient.DeleteTask(habitTask.Id);
                    }

                }
            }
        }
    }
}