#!/bin/bash
sed -i 's/\r$//' "$0"

export AWS_REGION=eu-west-2
export AWS_DEFAULT_REGION=eu-west-2
export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test

set -e

echo "Bootstrapping localstack starting..."

# Create SNS Topics
echo "Bootstrapping SNS setup..."

topic_arn=$(awslocal sns create-topic \
  --name ls-cads-cts-events \
  --endpoint-url=http://localhost:4566 \
  --output text \
  --query 'TopicArn')

echo "SNS Topic created: $topic_arn"
echo "Bootstrapping Complete"
