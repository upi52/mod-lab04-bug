using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;

namespace BugTests;

[TestClass]
public class UnitTest1
{

    [TestMethod]
    public void Bug_InitialState_IsNewDefect()
    {
        var bug = new Bug("T-001");
        Assert.AreEqual(State.NewDefect, bug.CurrentState);
    }

  
    [TestMethod]
    public void Bug_StartTriaging_MovesFromNewDefectToTriaging()
    {
        var bug = new Bug("T-002");
        bug.StartTriaging();
        Assert.AreEqual(State.Triaging, bug.CurrentState);
    }

   
    [TestMethod]
    public void Bug_SendToFix_MovesFromTriagingToFixing()
    {
        var bug = new Bug("T-003");
        bug.StartTriaging();
        bug.SendToFix();
        Assert.AreEqual(State.Fixing, bug.CurrentState);
    }

    
    [TestMethod]
    public void Bug_ProblemSolved_MovesFromFixingToClosed()
    {
        var bug = new Bug("T-004");
        bug.StartTriaging();
        bug.SendToFix();
        bug.ProblemSolved();
        Assert.AreEqual(State.Closed, bug.CurrentState);
    }


    [TestMethod]
    public void Bug_ProblemNotSolved_MovesFromFixingToReopened()
    {
        var bug = new Bug("T-005");
        bug.StartTriaging();
        bug.SendToFix();
        bug.ProblemNotSolved();
        Assert.AreEqual(State.Reopened, bug.CurrentState);
    }

   
    [TestMethod]
    public void Bug_ReturnToTriaging_MovesFromReopenedToTriaging()
    {
        var bug = new Bug("T-006");
        bug.StartTriaging();
        bug.SendToFix();
        bug.ProblemNotSolved();
        bug.ReturnToTriaging();
        Assert.AreEqual(State.Triaging, bug.CurrentState);
    }

   
    [TestMethod]
    public void Bug_NotDefect_MovesFromTriagingToClosed()
    {
        var bug = new Bug("T-007");
        bug.StartTriaging();
        bug.NotDefect();
        Assert.AreEqual(State.Closed, bug.CurrentState);
    }

   
    [TestMethod]
    public void Bug_NotReproducible_MovesFromTriagingToClosed()
    {
        var bug = new Bug("T-008");
        bug.StartTriaging();
        bug.NotReproducible();
        Assert.AreEqual(State.Closed, bug.CurrentState);
    }

    
    [TestMethod]
    public void Bug_Duplicate_MovesFromTriagingToClosed()
    {
        var bug = new Bug("T-009");
        bug.StartTriaging();
        bug.Duplicate();
        Assert.AreEqual(State.Closed, bug.CurrentState);
    }

    
    [TestMethod]
    public void Bug_NeedMoreInfo_StaysInTriaging()
    {
        var bug = new Bug("T-010");
        bug.StartTriaging();
        bug.NeedMoreInfo();
        Assert.AreEqual(State.Triaging, bug.CurrentState);
    }

    
    [TestMethod]
    public void Bug_ReturnToTriaging_MovesFromClosedToTriaging()
    {
        var bug = new Bug("T-011");
        bug.StartTriaging();
        bug.SendToFix();
        bug.ProblemSolved();
        bug.ReturnToTriaging();
        Assert.AreEqual(State.Triaging, bug.CurrentState);
    }

    
    [TestMethod]
    public void Bug_FullCycleWithReopen_EndsClosed()
    {
        var bug = new Bug("T-012");
        bug.StartTriaging();
        bug.SendToFix();
        bug.ProblemNotSolved();
        bug.ReturnToTriaging();
        bug.SendToFix();
        bug.ProblemSolved();
        Assert.AreEqual(State.Closed, bug.CurrentState);
    }

    
    [TestMethod]
    public void Bug_MultipleNeedMoreInfo_StaysInTriaging()
    {
        var bug = new Bug("T-013");
        bug.StartTriaging();
        bug.NeedMoreInfo();
        bug.NeedMoreInfo();
        bug.NeedMoreInfo();
        Assert.AreEqual(State.Triaging, bug.CurrentState);
    }

   
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bug_SendToFix_FromNewDefect_ThrowsException()
    {
        var bug = new Bug("T-014");
        bug.SendToFix();
    }

    
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bug_ProblemSolved_FromTriaging_ThrowsException()
    {
        var bug = new Bug("T-015");
        bug.StartTriaging();
        bug.ProblemSolved();
    }

   
    [TestMethod]
    public void Bug_CanFire_StartTriaging_FromNewDefect_ReturnsTrue()
    {
        var bug = new Bug("T-016");
        Assert.IsTrue(bug.CanFire(Trigger.StartTriaging));
    }

 
    [TestMethod]
    public void Bug_CanFire_SendToFix_FromNewDefect_ReturnsFalse()
    {
        var bug = new Bug("T-017");
        Assert.IsFalse(bug.CanFire(Trigger.SendToFix));
    }

 
    [TestMethod]
    public void Bug_Title_IsStoredCorrectly()
    {
        var title = "T-018: проверка заголовка";
        var bug = new Bug(title);
        Assert.AreEqual(title, bug.Title);
    }

   
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bug_SendToFix_FromClosed_ThrowsException()
    {
        var bug = new Bug("T-019");
        bug.StartTriaging();
        bug.NotDefect();
        bug.SendToFix();
    }

 
    [TestMethod]
    public void Bug_ReturnFromClosedAndCloseAgain_EndsClosed()
    {
        var bug = new Bug("T-020");
        bug.StartTriaging();
        bug.Duplicate();
        bug.ReturnToTriaging();
        bug.NotReproducible();
        Assert.AreEqual(State.Closed, bug.CurrentState);
    }
}
