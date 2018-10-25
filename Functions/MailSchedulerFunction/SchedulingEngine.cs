using MailSchedulerFunction.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailSchedulerFunction
{
    public class SchedulingEngine
    {
        private readonly Core.CoreDependencyInstances _coreDependencies;
        private readonly IDataSchedulerRepository _repository;

        public SchedulingEngine(Core.CoreDependencyInstances coreDependencies, IDataSchedulerRepository repository)
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
                return;
            }

            _coreDependencies.DiagnosticLogging.Verbose("Mail processing not in progress, attempting to schedule mail collection");
            //TODO: Lodge message to initiate mail collection
        }
    }
}
