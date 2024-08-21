using Pixed.Models;
using Pixed.Services.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Services
{
    internal class HistoryService
    {
        private List<HistoryEntry> _historyStates;
        private int _historyStateIndex = -1;

        public HistoryService()
        {
            _historyStates = [];
        }

        public void Add(HistoryEntry entry)
        {
            _historyStates.Add(entry);
            _historyStateIndex = _historyStates.Count - 1;
        }

        public void Undo()
        {
           
        }
    }
}
