using Npgsql;
using Straonit.HighEdge.Models;

namespace Straonit.HighEdge.Services;

public class TaskService
{
    private Dictionary<int, TaskData> tasks;

    private Random random;
    

    public TaskService()
    {        
        tasks = new Dictionary<int, TaskData>();
        random = new Random();
    }

    public async Task RunAsync(Func<Task> task, string taskType, string nodeHost, string nodeTarget)
    {
        var uid = random.Next(0, 10000);
        var taskObj = task.Invoke();
        var taskData = new TaskData()
        {
            StartTime = DateTime.UtcNow,            
            UID = uid,
            Task = taskObj,
            NodeTarget = nodeTarget,
            NodeHost = nodeHost,
            TaskType = taskType
        };
        tasks.Add(uid, taskData);
        taskObj.ContinueWith(f => {
            tasks.Remove(uid);
            // taskData.EndTime = DateTime.UtcNow;
        });
    }

    public TaskData GetTaskStatus(int taskId)
    {
        return tasks[taskId];
    }

    public IEnumerable<TaskData> GetTasks(){
        return tasks.Values.ToList();
    }
}