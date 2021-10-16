#! /bin/bash


###########################################################################
###  FUNCTIONS
##########################################################################


generateFeatureDomainUnitTests() {
  cd $PWD/tests/unit/$1.Domain.UnitTests
  cat > $2.cs <<EOF

EOF
}


generateFeatureContract() {
  echo 'Generating Feature Contract'
  cd $PWD/src/$1.Contract/features
  cat > $2.cs <<EOF

EOF
  cd $PWD
}




generateFeatureContractUnitTests() {
  cd $PWD/tests/unit/$1.Contract.UnitTests
  cat > $2.cs <<EOF
using $1.Schema;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace $1.Contract.UnitTests
{
    public static class $2
    {
        public class Tests : IoCTestsBase
        {
            private Contract.$2.Cmd _cmd;
            private Contract.$2.Evt _evt;
            private Contract.$2.Rsp _rsp;
            private Contract.$2.Req _req;


            public Tests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
            {
            }


            [Fact]
            public void Must_HaveCmd()
            {
                Assert.NotNull(_cmd);
            }

            [Fact]
            public void Must_HaveRsp()
            {
                Assert.NotNull(_rsp);
            }

            [Fact]
            public void Must_HaveEvt()
            {
                Assert.NotNull(_evt);
            }

            [Fact]
            public void Must_HaveReq()
            {
                Assert.NotNull(_req);
            }
            

            [Fact]
            public void Must_RspHaveSameCorrelationIdAsCmd()
            {
                Assert.Equal(_rsp.CorrelationId, _cmd.CorrelationId);
            }

            [Fact]
            public void Must_EvtHaveSameIdAsCmd()
            {
                Assert.Equal(_cmd.AggregateId, _evt.AggregateId);
            }

            [Fact]
            public void MustEvtHaveSameOrderAsCmd()
            {
                Assert.Equal(_cmd.Order, _evt.Order);
            }


            protected override void Initialize()
            {
                _cmd = new Contract.$2.Cmd(Aggregate.ID.New);
                _evt = Contract.$2.Evt.CreateNew(_cmd.AggregateId);
                _rsp = Contract.$2.Rsp.CreateNew(_cmd.CorrelationId, _cmd.AggregateId);
                _req = Contract.$2.Req.CreateNew(_cmd.CorrelationId);
            }


            protected override void SetTestEnvironment()
            {
            }

            protected override void InjectDependencies(IServiceCollection services)
            {
            }
        }
    }
}
EOF
}





generateDomainFeature() {
  echo 'Generating Feature Domain'
}





usage() {
  echo
  echo 
  echo 'Usage: ./add-feature.sh [OPTIONS]'
  echo
  echo '-n | --namespace    The Target Namespace Prefix (PascalCase)'
  echo '-f | --feature      Feature Name (PascalCase)' 
  echo
  echo '-h | --help         Usage' 
  echo 
  echo    
}



###########################################################################
###    MAIN
##########################################################################

namespace_prefix=
feature_name=

  while [ "$1" != "" ]; do
    case $1 in

       -n  | --namespace)    shift
                             namespace_prefix="$1"
                             ;;
       -f  | --feature)      shift
                             feature_name="$1"
                             ;;
       -m  | --model)        shift
                             model_name="$1" 
                             ;;
       -h  | --help)         usage
                             ;; 
        * )                  usage
                             ;;
    esac
    shift
  done

  clear

  if [[ "$namespace_prefix" != "" ]] && [[ "$feature_name" != "" ]] && [[ "$model_name" != "" ]] ; then
    generateContractConfig                        $namespace_prefix $feature_name $model_name
    #  generateFeatureContractUnitTests           $namespace_prefix $feature_name $model_name
    #  generateFeatureDomainUnitTests             $namespace_prefix $feature_name $model_name
    #  generateFeatureCmdInfraIntegrationTests    $namespace_prefix $feature_name $model_name
    #  generateFeatureEtlInfraIntegrationTests    $namespace_prefix $feature_name $model_name
    #  generateFeatureSubInfraIntegrationTests    $namespace_prefix $feature_name $model_name
  else
    usage
  fi



