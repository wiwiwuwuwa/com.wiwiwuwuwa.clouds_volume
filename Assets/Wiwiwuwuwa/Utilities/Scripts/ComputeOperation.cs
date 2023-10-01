using System;
using System.Collections;

namespace Wiwiwuwuwa.Utilities
{
    public abstract class ComputeOperation : IDisposable
    {
        // ----------------------------------------------------

        IEnumerator operation = default;

        Action disposion = default;

        // ----------------------------

        protected ComputeOperation()
        {
            operation = Execute();
            if (operation is null)
            {
                return;
            }

            disposion = () => { operation = default; };
        }

        // ----------------------------

        protected abstract IEnumerator Execute();

        // ----------------------------

        public bool MoveNext()
        {
            if (operation is null)
            {
                return false;
            }

            if (operation.MoveNext())
            {
                return true;
            }

            Dispose();
            return false;
        }

        public void Dispose()
        {
            disposion?.Invoke();
            disposion = default;
        }

        // ----------------------------

        protected void Defer(Action action)
        {
            disposion += action;
        }

        // ----------------------------------------------------
    }
}
