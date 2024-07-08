# Building the MessageRouter-web-client, because without it the chart couldn't be started.
cd $PSScriptRoot\..\..\..\..\..\..
npx nx run-many --target=build --projects=@morgan-stanley/composeui-messaging-client,@morgan-stanley/composeui-example-chart --maxParallel=100
