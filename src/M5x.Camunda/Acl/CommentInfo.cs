using System;
using Camunda.Api.Client.UserTask;

namespace M5x.Camunda.Acl;

public class CommentInfo : Comment
{
    /// <summary>
    ///     The id of the task comment.
    /// </summary>
    public string Id;

    /// <summary>
    ///     The id of the task to which the comment belongs.
    /// </summary>
    public string TaskId;

    /// <summary>
    ///     The time when the comment was created.
    /// </summary>
    public DateTime Time;

    /// <summary>
    ///     The id of the user who created the comment.
    /// </summary>
    public string UserId;

    public override string ToString()
    {
        return base.ToString() ?? Id;
    }
}