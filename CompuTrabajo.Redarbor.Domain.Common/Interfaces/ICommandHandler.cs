namespace CompuTrabajo.Redarbor.Application.Common.Interfaces
{
    public interface ICommandHandler<T> where T : ICommand
    {
        Task HandleAsync(T command, CancellationToken cancellationToken);
    }
}
