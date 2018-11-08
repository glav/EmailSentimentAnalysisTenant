using MailSchedulerFunction.Data;
using System;
using System.Threading.Tasks;

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

        public async Task ScheduleMailCollectionIfNotInProgressAsync()
        {
            _coreDependencies.DiagnosticLogging.Verbose("Attempting to schedule mail collection.");
            try
            {
                var mailProcessingInProgress = await _repository.IsMailOperationInProgressAsync();
                if (mailProcessingInProgress)
                {
                    _coreDependencies.DiagnosticLogging.Info("Mail processing currently in progress, skipping mail collection");
                    return;
                }
            }
            catch (Exception ex)
            {
                _coreDependencies.DiagnosticLogging.Error(ex, "Error checking if Mail Operation in progress");
            }

            _coreDependencies.DiagnosticLogging.Verbose("Mail processing not in progress, attempting to schedule mail collection");
            await _repository.SetMailOperationToInProgressAsync();
        }
    }
}
