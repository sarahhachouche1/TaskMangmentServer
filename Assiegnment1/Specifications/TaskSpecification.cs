using ServerSide.Models;
using System.Linq.Expressions;

namespace ServerSide.Specifications;

public abstract class TaskSpecification<T>
{
    public bool IsSatisfiedBy(T input, TaskToDo task )
    {
        Func<T, TaskToDo, bool> predicate = ToExpression().Compile();
        return predicate(input, task);
    }
    public abstract Expression<Func<T,TaskToDo , bool>> ToExpression();   


}

public sealed class TaskPrioritySpecification : TaskSpecification<Priority>
{
    public override Expression<Func<Priority, TaskToDo, bool>> ToExpression()
    {
        return (priority, task) => task.Priority == priority; 
    }
}

public sealed class TaskStatusSpecification : TaskSpecification<Status>
{
    public override Expression<Func<Status, TaskToDo, bool>> ToExpression()
    {
        return (status, task) => task.Status == status;
    }
}

public sealed class TaskStartDateSpecification : TaskSpecification<DateTime>
{
    public override Expression<Func<DateTime, TaskToDo, bool>> ToExpression()
    {
        return (startdate, task) => task.StartDate == startdate;
    }
}

public sealed class TaskEndDateSpecification : TaskSpecification<DateTime>
{
    public override Expression<Func<DateTime, TaskToDo, bool>> ToExpression()
    {
        return (enddate, task) => task.EndDate == enddate;
    }
}

public sealed class TaskTitleSpecification : TaskSpecification<string>
{
    public override Expression<Func<string, TaskToDo, bool>> ToExpression()
    {
        return (title, task) => task.Title == title;
    }
}