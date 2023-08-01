using XFrame.Common.Extensions;

namespace XFrame.Rules.Notifications
{
    public class Message : IMessage
    {
        #region Constructors

        public Message(string text)
            : this()
        {
            if(text.IsNullOrEmpty()) throw new ArgumentNullException("text");
            Text = text;
        }

        public Message(string text, SeverityType severityType)
            : this(text)
        {
            Severity = severityType;
        }

        public Message()
        {
            Severity = SeverityType.Information;
            MessageType = MessageType.None;
            MayOverrideMessage = true;
            ClassRuleTags = new Dictionary<string, IEnumerable<string>>();
        }

        #endregion

        #region Properties

        public bool MayOverrideMessage { get; set; }

        #endregion

        #region Virtual Methods

        public virtual string ToString(bool detailed)
        {
            if (detailed)
            {
                return "Message:{0}, Severity:{1}, RuleInfo: {2}"
                    .FormatInvariantCulture(Text, Severity, RuleInfo);
            }

            return Text;
        }

        #endregion

        #region Methods

        public Message WithClassTag(string tag, params string[] properties)
        {
            if (tag.IsNotNullOrEmpty())
            {
                ClassRuleTags.Add(tag, properties.ToList());
            }
            return this;
        }

        #endregion

        #region IMessage Members

        public string Text { get; set; }

        public SeverityType Severity { get; set; }

        public MessageType MessageType { get; set; }

        public string SystemReference { get; set; }

        public string Tag { get; set; }

        public string PropertyName { get; set; }

        public string RuleInfo { get; set; }

        public IDictionary<string, IEnumerable<string>> ClassRuleTags { get; set; }

        #endregion
    }
}
