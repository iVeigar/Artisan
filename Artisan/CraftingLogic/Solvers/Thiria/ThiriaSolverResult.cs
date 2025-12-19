namespace ThiriaExpertSolver.Thiria;

public class ThiriaSolverResult
{
    public bool done { get; set; }
    public bool error { get; set; }
    public required string message { get; set; }
    public string actionName { get; set; } = null!;
}
