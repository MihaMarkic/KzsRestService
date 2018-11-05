using System;
using System.Threading.Tasks;

namespace KzsRest.Engine
{
    public readonly struct HtmlRequest
    {
        readonly TaskCompletionSource<string> tcs;
        public Task<string> Task => tcs.Task;
        public string RelativeAddress { get; }
        public string[] Args { get; }
        public HtmlRequest(string relativeAddress, string[] args)
        {
            tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            RelativeAddress = relativeAddress;
            Args = args;
        }
        public void SetCanceled()
        {
            tcs.SetCanceled();
        }
        public void SetCompleted(string result)
        {
            tcs.SetResult(result);
        }
        public void SetException(Exception ex)
        {
            tcs.SetException(ex);
        }
    }
}
