using Defolderizer.Interfaces;

namespace Defolderizer.Tests.Mocs {
    public class TestLoggingService : ILoggingService {
        
        public string WriteLogEntry(string userLogText, string developerLogText = "") {
            return "";
        }
    }
}
