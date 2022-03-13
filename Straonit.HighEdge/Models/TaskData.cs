namespace Straonit.HighEdge.Models;

public class TaskData
{
    public int UID { get; set; }
    public Task Task { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string NodeHost { get; set; }
    public string NodeTarget { get; set; }
    public string TaskType { get; set; }
}