export interface User {
    username: string,
    firstname: string,
    lastname: string,
    email: string,
    permissions: number
  }
  
  export interface Review {
    movieid: string,
    username: string,
    rating: string,
    text: string
  }
  
  export interface Discussion {
    discussionid: number,
    movieid: string,
    username: string,
    subject: string,
    topic: string
  }
  
  export interface Comment {
    commentid: number,
    discussionid: number,
    username: string,
    text: string,
    isspoiler: boolean
  }
  