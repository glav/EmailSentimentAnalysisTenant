using MailSchedulerFunction.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailSchedulerFunction
{
    public class SchedulingEngine
    {
        private readonly Core.DependencyInstances _coreDependencies;
        private readonly IDataSchedulerRepository _repository;

        public SchedulingEngine(Core.DependencyInstances coreDependencies, IDataSchedulerRepository repository)
        {
            _coreDependencies = coreDependencies;
            _repository = repository;
        }

        public void ScheduleMailCollectionIfNotInProgress()
        {
            _coreDependencies.DiagnosticLogging.Info("Attempting to schedule mail collection.");
            var mailProcessingInProgress = _repository.IsMailOperationInProgress();
            if (mailProcessingInProgress)
            {
                _coreDependencies.DiagnosticLogging.Info("Mail processing currently in progress, skipping mail collection");
            }

            _coreDependencies.DiagnosticLogging.Verbose("Mail processing not in progress, attempting to schedule mail collection");
        }
    }
}
