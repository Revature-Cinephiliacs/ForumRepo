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
    topic: string
  }

  export interface Comment {
    commentid: number,
    discussionid: number,
    userid: string,
    text: string,
    isspoiler: boolean,
    parentcommentid: string
  }

  