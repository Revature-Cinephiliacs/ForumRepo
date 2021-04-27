export interface User {
    username: string,
    firstname: string,
    lastname: string,
    email: string,
    permissions: number
  }
  
  export interface Review {
    movieid: string,
    userid: string,
    rating: string,
    text: string
  }
  
  export interface Discussion {
    discussionid: number,
    movieid: string,
    userid: string,
    subject: string,
    topic: string,
    comments: [Comment]
  }

  export interface Comment {
    commentid: number,
    discussionid: number,
    userid: string,
    text: string,
    isspoiler: boolean,
    parentcommentid: string
  }

  export class NestedComment {
    constructor(
      public commentid: number,
      public discussionid: number,
      public userid: string,
      public text: string,
      public isspoiler: boolean,
      public parentcommentid: string,
      public replies: any
    ){}
  }
  