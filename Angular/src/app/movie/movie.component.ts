import { Component, OnInit } from '@angular/core';
import { ForumService} from '../forum.service';
import { ActivatedRoute } from '@angular/router';
import { Discussion } from '../models';


@Component({
  selector: 'app-movie',
  templateUrl: './movie.component.html',
  styleUrls: ['./movie.component.css']
})
export class MovieComponent implements OnInit {

  constructor( private _forum: ForumService, private router:  ActivatedRoute) { }

  movieID: string = this.router.snapshot.params.id;
  topics:any;
  discussions: any;
  comments: any;
  commentCount: any;
  newDiscussions = [];
  displayMoreDuscussion: boolean = false;
  newDiscussion: Discussion & {commentCount: number};
  submitDiscussion: any ={
    movieid: this.router.snapshot.params.id,
    topic:"",
    username:"sagar1",
    subject:""
  }


  ngOnInit(): void {

    this._forum.getTopics().subscribe(data => {
      console.log(data);
      this.topics = data;
    });
    this.getDiscussions();

  }

  postDiscussion(){
    if(this.submitDiscussion.topic == "" || this.submitDiscussion.subject == "")
    {
      console.log("didn't submit discussion");
    }else if(this.submitDiscussion.subject.length >= 250){
      alert("Discussion should be less than 250 Characters")
    }else{

      this._forum.submitDiscussion(this.submitDiscussion).subscribe(data => console.log(data));
      //this.showDiscussion();
    }
    console.log(this.submitDiscussion);
  }

  async getDiscussions() {
    setTimeout(() => {
      this._forum.getDiscussion(this.router.snapshot.params.id).subscribe(data => {
        console.log(data);
        
        this.discussions = data;
        if(this.discussions.length > 5){
          this.discussions =  this.discussions.slice(0, 5)
          this.displayMoreDuscussion = true;
        }
        this.discussions.forEach(d => {
          d.discussionid;
          this.addCommentCount(d);
        });    
      });
    }, 10);
  }

  async addCommentCount(d: any) {
    setTimeout(() => {
      this._forum.getDiscussionComments(d.discussionid).subscribe(data =>{ 
        this.comments = data;
        if(this.comments == null)
        {
          this.commentCount = 0;
        }
        else
        {
          this.commentCount = this.comments.length;
        }
        this.newDiscussion = {
          discussionid: d.discussionid,
          movieid: d.movieid,
          username: d.username,
          subject: d.subject,
          topic: d.topic,
          commentCount: this.commentCount
        }
        this.newDiscussions.push(this.newDiscussion);
      });
    }, 1000);
  }

}
