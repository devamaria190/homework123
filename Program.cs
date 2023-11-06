using System;
using System.Collections.Generic;

// 1. Дженеричний калькулятор
public class Calculator<T>
{
    public delegate T Operation(T a, T b);

    public Operation Add { get; set; }
    public Operation Subtract { get; set; }
    public Operation Multiply { get; set; }
    public Operation Divide { get; set; }
}

// 2. Дженеричний репозиторій з критеріями
public class Repository<T>
{
    private List<T> data = new List<T>();

    public delegate bool Criteria<T>(T item);

    public List<T> Find(Criteria<T> criteria)
    {
        List<T> result = new List<T>();
        foreach (var item in data)
        {
            if (criteria(item))
            {
                result.Add(item);
            }
        }
        return result;
    }
}

// 3. Дженеричний кеш з викликами користувацьких функцій
public class FunctionCache<TKey, TResult>
{
    private Dictionary<TKey, TResult> cache = new Dictionary<TKey, TResult>();
    private Func<TKey, TResult> function;

    public FunctionCache(Func<TKey, TResult> function)
    {
        this.function = function;
    }

    public TResult Execute(TKey key)
    {
        if (cache.ContainsKey(key))
        {
            return cache[key];
        }
        else
        {
            TResult result = function(key);
            cache.Add(key, result);
            return result;
        }
    }
}

// 4. Дженеричний планувальник завдань
public class TaskScheduler<TTask, TPriority>
{
    private Queue<TTask> tasks = new Queue<TTask>();
    private Func<TTask, TPriority> getPriority;
    private Action<TTask> resetTask;

    public TaskScheduler(Func<TTask, TPriority> getPriority, Action<TTask> resetTask)
    {
        this.getPriority = getPriority;
        this.resetTask = resetTask;
    }

    public void AddTask(TTask task)
    {
        tasks.Enqueue(task);
    }

    public void ExecuteNext(TaskExecution<TTask> execution)
    {
        if (tasks.Count > 0)
        {
            TTask task = tasks.Dequeue();
            execution(task);
            resetTask(task);
        }
        else
        {
            Console.WriteLine("No tasks to execute.");
        }
    }

    public void ReturnToPool(TTask task)
    {
        resetTask(task);
        tasks.Enqueue(task);
    }
}

// Метод для виконання завдання
public delegate void TaskExecution<TTask>(TTask task);

// Приклад використання
class Program
{
    static void Main(string[] args)
    {
        // Приклад для калькулятора
        var intCalculator = new Calculator<int>();
        intCalculator.Add = (a, b) => a + b;
        intCalculator.Subtract = (a, b) => a - b;
        intCalculator.Multiply = (a, b) => a * b;
        intCalculator.Divide = (a, b) => b != 0 ? a / b : throw new DivideByZeroException();

        Console.WriteLine(intCalculator.Add(5, 3));
        Console.WriteLine(intCalculator.Divide(10, 2));

        // Приклад для репозиторія
        var repository = new Repository<int>();
        repository.Add(5);
        repository.Add(10);
        repository.Add(15);
        var result = repository.Find(x => x > 7);
        foreach (var item in result)
        {
            Console.WriteLine(item);
        }

        // Приклад для кешу функцій
        Func<string, int> stringLengthFunc = (str) => str.Length;
        var cache = new FunctionCache<string, int>(stringLengthFunc);
        Console.WriteLine(cache.Execute("Hello"));

        // Приклад для планувальника завдань
        var taskScheduler = new TaskScheduler<string, int>(
            (task) => task.Length,
            (task) => Console.WriteLine($"Resetting task: {task}")
        );
        taskScheduler.AddTask("Task 1");
        taskScheduler.AddTask("Task 2");
        taskScheduler.ExecuteNext((task) => Console.WriteLine($"Executing task: {task}"));
    }
}