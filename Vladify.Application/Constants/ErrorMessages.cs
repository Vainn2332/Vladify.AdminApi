namespace Vladify.Application.Constants;

public static class ErrorMessages
{
    public const string TaskNotFoundById = "Task with such id not found!";

    public const string TaskAssignedToDifferentModerator = "Can't modify task that is already assigned to different moderator!";

    public const string AlreadyHasActiveTask = "You can't have more than 1 active task at the same time!";

    public const string TaskNotClaimed = "You can't approve/reject task that isn't assigned to anyone!";
}
