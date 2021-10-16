// The MIT License (MIT)
// 
// Copyright (c) 2019-2021 Macula Team 
// Copyright (c) 2019-2021 DisComCo Sp.z.o.o
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using EventFlow.Core;

namespace M5x.CEQS
{
    public interface IActor<TAggregate, TID, TCommand> : 
        ICommandHandler<TAggregate, TID, IExecutionResult, TCommand> 
        where TAggregate : IAggregateRoot<TID> where TID : IIdentity where TCommand : ICommand<TAggregate, TID, IExecutionResult>
    {
        
    }


    public abstract class Actor<TAggregate,TIdentity, TCommand> :
        CommandHandler<TAggregate, TIdentity, IExecutionResult, TCommand>,
        IActor<TAggregate, TIdentity, TCommand> where TIdentity : IIdentity 
        where TCommand : ICommand<IAggregateRoot<TIdentity>, TIdentity, IExecutionResult>, ICommand<TAggregate, TIdentity, IExecutionResult>
        where TAggregate : IAggregateRoot<TIdentity>
    {

        public override Task<IExecutionResult> ExecuteCommandAsync(TAggregate aggregate, TCommand command, CancellationToken cancellationToken)
        {
            IExecutionResult result = ((dynamic)aggregate).Execute((dynamic)command); 
            return Task.FromResult(result);
        }
    }
}