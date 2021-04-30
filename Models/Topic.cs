using System;

namespace GlobalModels
{
    public class Topic
    {
        /// <summary>
        /// DTO model for Topic
        /// </summary>
        /// <value></value>
        public Guid TopicId { get; set; }
        public string TopicName { get; set; }

        public Topic(string topicid, string topicName)
        {
            TopicId = Guid.Parse(topicid);
            TopicName = topicName;
        }

    }
}