namespace ChatSupport.API.Models
{
    public class AgentLoadDto
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public int CurrentChatCount { get; set; }
        public int MaxConcurrency { get; set; }
        public bool CanAcceptMoreChats { get; set; }
    }
}
