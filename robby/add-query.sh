#! /bin/bash

generateContractFeature() {

}

generateDomainFeature() {

}

###########################################################################
###    MAIN
##########################################################################

  while [ "$1" != "" ]; do
    case $1 in

       -n  | --name )        shift
                             feature_name="$1"
                             ;;
       -u  | --user )        shift
                             user="$1"
                             ;;
       -p  | --password)     shift
                             password="$1"
                             ;;
       -i  | --image)        shift
                             img_prefix="$1"
                             ;;
       -ip | --id-prefix)    shift
                             id_prefix="$1"
                             ;;
       -s  | --sdk-version)  shift
                             sdk_version="$1"
                             ;;
       -h  | --help)         usage
                             ;; 
        * )                  usage
                             ;;
    esac
    shift
  done

  clear

  if [[ "$user" != "" ]] && [[ "$password" != "" ]]  && [[ "$api_prefix" != "" ]] ; then