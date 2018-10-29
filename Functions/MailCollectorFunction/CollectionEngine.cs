using Core;
using MailCollectorFunction.Config;
using MailCollectorFunction.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MailCollectorFunction
{
    class CollectionEngine
    {
        private readonly CoreDependencyInstances _coreDependencies;
        private readonly IMailCollectionRepository _repository;
        private readonly EmailConfiguration _mailConfig;
        public CollectionEngine(CoreDependencyInstances coreDependencies, IMailCollectionRepository repository, EmailConfiguration mailConfig)
        {
            _coreDependencies = coreDependencies;
            _repository = repository;
            _mailConfig = mailConfig;
        }

        public async Task PerformMailCollectionAsync()
        {
            var emails = await _repository.CollectMailAsync(_mailConfig);
            await _repository.StoreMailAsync(emails);
            //TODO: Send message that collection is complete
            return;
        }
    }
}
