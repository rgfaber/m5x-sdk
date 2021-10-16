#! /bin/bash
clear
echo
echo '------------------------------------'
echo pushing version $1 to develop branch
echo '------------------------------------'
echo
git checkout develop
git push --set-upstream origin develop

echo  
echo
echo 'cooling down for 30 seconds..'
sleep 30s
echo
echo
clear
echo '-----------------------------------'
echo pushing version "$1" to main branch
echo '-----------------------------------'
git checkout main
git add .
git commit -m "$1" -a

git merge develop
git push
git checkout develop