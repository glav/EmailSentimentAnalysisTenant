﻿using Core;
using Core.Data;
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

        public async Task PerformMailCollectionAsync(GenericActionMessage receivedMessage)
        {
            _coreDependencies.DiagnosticLogging.Info("Performing Mail Collection:{0}", receivedMessage);

            try
            {
                var emails = await _repository.CollectMailAsync(_mailConfig);
                await _repository.StoreMailAsync(emails);
            } catch (Exception ex)
            {
                _coreDependencies.DiagnosticLogging.Fatal(ex, "Error performing mail collection");
            }

            try
            {
                await _repository.LodgeMailCollectedAcknowledgementAsync(receivedMessage);
                _coreDependencies.DiagnosticLogging.Info("Completed performing mail collection");
            } catch (Exception ex)
            {
                _coreDependencies.DiagnosticLogging.Fatal(ex, "Could not Lodge mail collection acknowledgement. Manual intervention required");
            }

            return;
        }
    }
}
