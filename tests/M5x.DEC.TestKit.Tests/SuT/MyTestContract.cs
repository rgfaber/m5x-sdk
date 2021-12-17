namespace M5x.DEC.TestKit.Tests.SuT;

public static class MyTestContract
{
    public static MyHope Hope = MyBogus.Contract.Hope.Generate();
    public static MyFeedback Feedback = MyBogus.Contract.Feedback.Generate();
    public static MyPagedQry PagedQry = MyBogus.Contract.PagedQuery.Generate();
    public static MySingletonQuery SingletonQuery = MyBogus.Contract.SingletonQuery.Generate();

    public static MyFact Fact = MyBogus.Contract.Fact.Generate();
}