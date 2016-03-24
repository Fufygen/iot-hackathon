namespace Shared
{
    public interface IMotor
    {
        int Id { get; }

        bool IsMoving { get; }

        void Backward();
        void Forward();
        void Stop();
    }
}