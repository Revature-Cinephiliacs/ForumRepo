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
  
  displayMoreDuscussion: boolean = false;
  
  submitDiscussion: any = {
    movieid: this.router.snapshot.params.id,
    topic:"",
    userid:"b23dbdad-3179-4b9a-b514-0164ee9547f3",
    subject:""
  }


  ngOnInit(): void {
    this._forum.getTopics().subscribe(data => {
      console.log(data);
      this.topics = data;
    });
    this.getDiscussions();
  }

  //Function that will add a new discussion to a movie
  //Will validate input
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

  //Function that will get a list of discussions for a movie, and 
  //Slice the results to only display the first five
  //---------------------------------------------
  //Might make handling top 5 in backend instead of frontend
  //(Api will return top 5, instead of slicing in frontend)
  //---------------------------------------------
  async getDiscussions() {
    setTimeout(() => {
      this._forum.getDiscussion(this.router.snapshot.params.id).subscribe(data => {
        this.discussions = data;
        console.log(data);
        console.log(this.discussions[1].comments.length);
        if(this.discussions.length > 5){
          this.discussions =  this.discussions.slice(0, 5)
          this.displayMoreDuscussion = true;
        }
          
      });
    }, 10);
  }

}
