﻿using System;
using WinApi.User32;

namespace WinApi.XWin
{
    public interface IEventLoop
    {
        int Run();
    }

    public class EventLoop : IEventLoop
    {
        public int Run()
        {
            Message msg;
            int res;
            while ((res = User32Methods.GetMessage(out msg, IntPtr.Zero, 0, 0)) != 0)
            {
                User32Methods.TranslateMessage(ref msg);
                User32Methods.DispatchMessage(ref msg);
            }
            return res;
        }
    }

    public class RealtimeEventLoop : IEventLoop
    {
        public int Run()
        {
            Message msg;
            var quitMsg = (uint) WM.QUIT;
            do
            {
                if (User32Helpers.PeekMessage(out msg, IntPtr.Zero, 0, 0, PeekMessageFlags.PM_REMOVE) != 0)
                {
                    User32Methods.TranslateMessage(ref msg);
                    User32Methods.DispatchMessage(ref msg);
                }
            } while (msg.Value != quitMsg);
            return 0;
        }
    }

    public class InterceptableEventLoop : IEventLoop
    {
        public virtual int Run()
        {
            Message msg;
            int res;
            while ((res = User32Methods.GetMessage(out msg, IntPtr.Zero, 0, 0)) != 0)
                if (Preprocess(ref msg))
                {
                    User32Methods.TranslateMessage(ref msg);
                    if (PostTranslate(ref msg))
                        User32Methods.DispatchMessage(ref msg);
                    PostProcess(ref msg);
                }
            return res;
        }

        protected bool Preprocess(ref Message msg) => true;
        protected bool PostTranslate(ref Message msg) => true;
        protected void PostProcess(ref Message msg) {}
    }

    public class InterceptableRealtimeEventLoop : IEventLoop
    {
        public virtual int Run()
        {
            Message msg;
            var quitMsg = (uint) WM.QUIT;
            do
            {
                if (User32Helpers.PeekMessage(out msg, IntPtr.Zero, 0, 0, PeekMessageFlags.PM_REMOVE) != 0)
                    if (Preprocess(ref msg))
                    {
                        User32Methods.TranslateMessage(ref msg);
                        if (PostTranslate(ref msg))
                            User32Methods.DispatchMessage(ref msg);
                        PostProcess(ref msg);
                    }
            } while (msg.Value != quitMsg);
            return 0;
        }

        protected bool Preprocess(ref Message msg) => true;
        protected bool PostTranslate(ref Message msg) => true;
        protected void PostProcess(ref Message msg) {}
    }
}