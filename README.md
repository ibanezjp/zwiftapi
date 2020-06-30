# Zwift API using Azure Functions

The PROBLEM: We are a group of friend doing indoor cycling using Zwift (Onliny Cycling and Running APP in virtual worlds). For each completed route Zwift gives you a badge of "Route Completed". We what to know which uncompleted route we have in common? Zwift DOES NOT have an API to access information.

I created some Azure Functions to solve this problem and also created a Telegram bot to access the information from our phones.

I created a webscraper to download routes from https://zwifthacks.com/app/routes/.

To import your completed route I created two options:
#### Manual import one by one.
1. HttpTrigger Azure Functions to add/remove completed routes.
#### Auto import from a screen shot.
1. Upload screenshot to Azure Storage (BLOB)
2. An Azure Function triggered by the new BLOB identifies completed routes (using https://www.customvision.ai/) and then identifies the name of completed routes (using https://cloud.google.com/vision/docs/ocr) and stored the results in a queue.
3. An Azure Function triggered by the new message in the Queue marks completed routes as DONE in your profile.

![Badges](https://zwfitstorageaccount.blob.core.windows.net/train/Chapas_4.jpeg)

User data is stored in Cosmos DB (just for learning porpouses)

Also there are other HttpTrigger Azure Functions to get pending routes.

This API is consumed by an Telegram Bot that I created using Azure Bot Service.

