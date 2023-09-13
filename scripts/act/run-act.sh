# NOTE: Run this in bash shell from the root repository directory with the command: ./scripts/act/run-act.sh
#       Don't forget to add .secrets and .env files on the paths below.
#       For more info on how to set up act, see https://github.com/nektos/act

# NOTE: act and SonarCloud don't really work well together. Therefore, we're forced to use the 'workflow_dispatch' event command for running act locally.
#       https://community.sonarsource.com/t/sonarcloud-github-action-not-working-in-github-enterprise-self-hosted-runners/81168
#       https://community.sonarsource.com/t/sonarcloud-github-action-incompatibility-with-act-github-actions-local-runner/54074
# 
#       To keep things clean, always make sure that you're running act on a short-living branch (i.e. not 'main' or 'dev') by configuring the IS_TEST_RUNNER environment variable.
#       

# NOTE: The -r flag is set to reuse action containers to maintain state.
#       This is because (for some reason), the caching action doesn't work properly together with act.
#       See https://github.com/nektos/act/issues/285

echo "Running act in directory:"
pwd
act workflow_dispatch -r --secret-file ./scripts/act/.secrets --env-file ./scripts/act/.env
