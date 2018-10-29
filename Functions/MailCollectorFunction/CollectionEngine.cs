using Core;
using MailCollectorFunction.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailCollectorFunction
{
    class CollectionEngine
    {
        private readonly CoreDependencyInstances _coreDependencies;
        private readonly IMailCollectionRepository _repository;
        public CollectionEngine(CoreDependencyInstances coreDependencies, IMailCollectionRepository repository)
        {
            _coreDependencies = coreDependencies;
            _repository = repository;
        }
    }
}
