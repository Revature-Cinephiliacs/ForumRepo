# Cinephiliacs Forum Microservice

## Description
This microservice is part of the Cinephiliacs application. It manages all forum data, such as discussion and comments. The endpoints available create, query, or manipulate that data.

## Technologies Used
* .NET 5
* ASP.NET Core
* EntityFramework Core

## Usage
#### API Address
http://20.45.6.142/
#### Endpoint Objects
* Discussion
  * discussionid: string,
  * movieid: string,
  * username: string,
  * subject: string,
  * topic: string

* NewDiscussion
  * movieid: string,
  * username: string,
  * subject: string,
  * topic: string

* Comment
  * commentid: string,
  * discussionid: string,
  * username: string,
  * text: string,
  * isspoiler: boolean

* NewComment
  * discussionid: string,
  * username: string,
  * text: string,
  * isspoiler: boolean

##### Object usage within an endpoint is denoted by placing the object name with parenthesis: (Object)
#### Endpoints
| Description                       | Type | Path                                                | Request Body    | Returned       | Comments                               |
|-----------------------------------|------|-----------------------------------------------------|-----------------|----------------|----------------------------------------|
| Get all topics                    | Get  | forum/topics                                        |                 | string[]       |                                        |
| Create a new topic                | Post | forum/topic/topic                                   | string          |                |                                        |
| Get movie's discussions           | Get  | forum/discussions/{movieid}                         |                 | (Discussion)[] | Returns an array of Discussion objects |
| Get discussion by id              | Get  | forum/discussion/{discussionid}                     |                 | (Discussion)   |                                        |
| Get sorted discussion by id       | Get  | forum/discussion/{movieid}/{page}/{sortingOrder}    |                 | (Discussion)[] |                                        |
| Get discussion's comments         | Get  | forum/comments/{discussionid}                       |                 | (Comment)[]    | Returns an array of Comment objects    |
| Get discussion's comments by page | Get  | forum/comments/{discussionid}/{page}/{sortingOrder} |                 | (Comment)[]    | Returns an array of Comment objects    |
| Set page size for comments        | Post | forum/comments/page/{pagesize}                      |                 |                |                                        |
| Create new Discusssion            | Post | forum/discussion                                    | (NewDiscussion) |                |                                        |
| Get discussions sorted by recent  | Post | forum/discussion/sort/comment/recent                |                 | (Discussion)[] |                                        |
| Get all discussions with topic    | Get  | forum/discussions/topic/{topicid}                   |                 | (Discussion)[] |                                        |
