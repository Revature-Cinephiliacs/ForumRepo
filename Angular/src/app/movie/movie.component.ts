import { Component, OnInit } from '@angular/core';
import { ForumService} from '../forum.service';


@Component({
  selector: 'app-movie',
  templateUrl: './movie.component.html',
  styleUrls: ['./movie.component.css']
})
export class MovieComponent implements OnInit {

  constructor( private _user: ForumService) { }

  topics:any;
  submitDiscussion: any ={
    movieid: "Ironman",
    topic:"",
    username:"sagar1",
    subject:""
  }


  ngOnInit(): void {

    this._user.getTopics().subscribe(data => {
      console.log(data);
      this.topics = data;
    });
  }

  postDiscussion(){
    if(this.submitDiscussion.topic == "" || this.submitDiscussion.subject == "")
    {
      console.log("didn't submit discussion");
    }else if(this.submitDiscussion.subject.length >= 250){
      alert("Discussion should be less than 250 Characters")
    }else{

      this._user.submitDiscussion(this.submitDiscussion).subscribe(data => console.log(data));
      //this.showDiscussion();
    }
    console.log(this.submitDiscussion);
  }

}
