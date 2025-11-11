using System;
using System.Collections.Generic;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class StandardVersion
    {
        private int major;
        private int minor;
        private int patch;

        public StandardVersion(int major, int minor, int patch)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
        }
    }

    internal interface IStandard
    {
        string StandardCode { get; }
        StandardVersion Version { get; }
        string URL { get; }
    }

    internal abstract class BaseStandard : IStandard
    {
        public abstract string StandardCode { get; }
        public abstract StandardVersion Version { get; }

        public string URL =>
            $"https://standards.ds.unity3d.com/standards/{StandardCode}?version={Version}";
    }

    internal interface IStandardChecker : IStandard
    {
        List<StandardIssue> IssuesFound { get; }
    }

    enum StandardIssueType
    {
        Info,
        Warning,
        Error
    }

    internal class StandardIssue
    {
        //TODO: consider using error codes?
        public StandardIssueType Type;
        public string Message;
    }

    internal abstract class BaseStandardChecker : BaseStandard, IStandardChecker
    {
        List<StandardIssue> issues = new List<StandardIssue>();

        protected void AddError(string message)
        {
            issues.Add(new StandardIssue { Message = message, Type = StandardIssueType.Error });
        }

        protected void AddWarning(string message)
        {
            issues.Add(new StandardIssue { Message = message, Type = StandardIssueType.Warning });
        }

        protected void AddInformation(string message)
        {
            issues.Add(new StandardIssue { Message = message, Type = StandardIssueType.Info });
        }

        public List<StandardIssue> IssuesFound => issues;
    }

    internal interface IStandardCondition : IStandard
    {
        //USC code
        bool Check();
    }
}
