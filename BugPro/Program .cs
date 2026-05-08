using Stateless;

namespace BugPro;


public enum State
{
    NewDefect,      
    Triaging,       
    Fixing,         
    Reopened,       
    Closed         
}


public enum Trigger
{
    StartTriaging,      
    SendToFix,          
    NotDefect,          
    NotReproducible,    
    Duplicate,          
    NeedMoreInfo,     
    ProblemSolved,      
    ProblemNotSolved,   
    ReturnToTriaging,   
}


public class Bug
{
    private readonly StateMachine<State, Trigger> _machine;
    private readonly string _title;

    public State CurrentState => _machine.State;
    public string Title => _title;

    public Bug(string title, State initialState = State.NewDefect)
    {
        _title = title;
        _machine = new StateMachine<State, Trigger>(initialState);
        ConfigureStateMachine();
    }

    private void ConfigureStateMachine()
    {
        _machine.Configure(State.NewDefect)
            .Permit(Trigger.StartTriaging, State.Triaging)
            .OnEntry(() => Console.WriteLine($"[{_title}] Новый дефект зарегистрирован."));

        _machine.Configure(State.Triaging)
            .Permit(Trigger.SendToFix, State.Fixing)
            .Permit(Trigger.NotDefect, State.Closed)
            .Permit(Trigger.NotReproducible, State.Closed)
            .Permit(Trigger.Duplicate, State.Closed)
            .PermitReentry(Trigger.NeedMoreInfo)
            .OnEntry(() => Console.WriteLine($"[{_title}] Разбор дефекта..."));

        _machine.Configure(State.Fixing)
            .Permit(Trigger.ProblemSolved, State.Closed)
            .Permit(Trigger.ProblemNotSolved, State.Reopened)
            .OnEntry(() => Console.WriteLine($"[{_title}] Исправление..."));

        _machine.Configure(State.Reopened)
            .Permit(Trigger.ReturnToTriaging, State.Triaging)
            .OnEntry(() => Console.WriteLine($"[{_title}] Переоткрыт — возврат на разбор."));

        _machine.Configure(State.Closed)
            .Permit(Trigger.ReturnToTriaging, State.Triaging)
            .OnEntry(() => Console.WriteLine($"[{_title}] Закрыт."));
    }

    public void StartTriaging()    => _machine.Fire(Trigger.StartTriaging);
    public void SendToFix()        => _machine.Fire(Trigger.SendToFix);
    public void NotDefect()        => _machine.Fire(Trigger.NotDefect);
    public void NotReproducible()  => _machine.Fire(Trigger.NotReproducible);
    public void Duplicate()        => _machine.Fire(Trigger.Duplicate);
    public void NeedMoreInfo()     => _machine.Fire(Trigger.NeedMoreInfo);
    public void ProblemSolved()    => _machine.Fire(Trigger.ProblemSolved);
    public void ProblemNotSolved() => _machine.Fire(Trigger.ProblemNotSolved);
    public void ReturnToTriaging() => _machine.Fire(Trigger.ReturnToTriaging);

    public bool CanFire(Trigger trigger) => _machine.CanFire(trigger);

    public IEnumerable<Trigger> PermittedTriggers => _machine.GetPermittedTriggers();

    public override string ToString() =>
        $"Bug '{_title}': состояние = {CurrentState}";
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Демонстрация WorkFlow бага ===\n");

      
        Console.WriteLine("--- Сценарий 1: успешное исправление ---");
        var bug1 = new Bug("BUG-001: Краш при авторизации");
        Console.WriteLine(bug1);
        bug1.StartTriaging();
        bug1.SendToFix();
        bug1.ProblemSolved();
        Console.WriteLine(bug1);

        Console.WriteLine();

   
        Console.WriteLine("--- Сценарий 2: не дефект ---");
        var bug2 = new Bug("BUG-002: Кнопка не того цвета");
        bug2.StartTriaging();
        bug2.NotDefect();
        Console.WriteLine(bug2);

        Console.WriteLine();

       
        Console.WriteLine("--- Сценарий 3: переоткрытие после неудачного исправления ---");
        var bug3 = new Bug("BUG-003: Утечка памяти");
        bug3.StartTriaging();
        bug3.NeedMoreInfo();
        bug3.SendToFix();
        bug3.ProblemNotSolved();
        bug3.ReturnToTriaging();
        bug3.SendToFix();
        bug3.ProblemSolved();
        Console.WriteLine(bug3);

        Console.WriteLine();

   
        Console.WriteLine("--- Сценарий 4: возврат из закрытого ---");
        var bug4 = new Bug("BUG-004: Дублирующийся баг");
        bug4.StartTriaging();
        bug4.Duplicate();
        bug4.ReturnToTriaging();
        bug4.SendToFix();
        bug4.ProblemSolved();
        Console.WriteLine(bug4);
    }
}
