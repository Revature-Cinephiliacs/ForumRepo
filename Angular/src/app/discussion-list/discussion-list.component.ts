import { Component, OnInit } from '@angular/core';

import { ForumService } from '../forum.service';
import { ActivatedRoute } from '@angular/router';
import { Discussion } from '../models';
import { ThisReceiver } from '@angular/compiler';

@Component({
  selector: 'app-discussion-list',
  templateUrl: './discussion-list.component.html',
  styleUrls: ['./discussion-list.component.css']
})
export class DiscussionListComponent implements OnInit {

  discussions: any;
  searchDiscussions = [];
  newDiscussion: Discussion & {commentCount: number};
  newDiscussions = [];
  sortDiscussions: any;
  comments: any;
  commentCount: any;
  movieID:string = "";
  discussionID: any;
  DisplayList: boolean = true;

  pageNum: number = 1;
  sortingOrder: string = "commentsD"   //Default sorting order will be based on total num of comments 
  numOfDiscussion: number;

  constructor(private _forum: ForumService, private router:  ActivatedRoute) { }

  ngOnInit(): void {
    // this.DisplayList = true;
    // this.newDiscussions = [];
  
    this.movieID =  this.router.snapshot.params.id;
    console.log("Discussion List:" + this.movieID);
    this.getDiscussions();
    this._forum.getDiscussion(this.movieID).subscribe(data =>{ 
      this.discussions = data;
      this.numOfDiscussion = this.discussions.length
      this.discussions = []})
  }

  //Function that will get a list of discussions associated with the
  //snapshot movie id
  async getDiscussions() {
    setTimeout(() => {
      this._forum.getDiscussionPage(this.movieID, this.pageNum, this.sortingOrder).subscribe(data => {
        console.log(data);
        
        this.discussions = data;
        
        this.discussions.forEach(d => {
          this.addCommentCount(d);
        });    
      });
    }, 1000);
  }

  //get next discussion page
  onNext(){
    this.newDiscussions = [];
    this.pageNum++;
    this.getDiscussions();
  }
  //get previous duscussuin page
  onPrev(){
    this.newDiscussions = [];
    this.pageNum--;
    this.getDiscussions();
  }

  //Function that will take in a discussion object and will
  //get the number of comments and add it to a new
  //discussion object with an added property for comment count, which is then 
  //added to a discussion list
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
          userid: d.userid,
          subject: d.subject,
          topic: d.topic,
          commentCount: this.commentCount
        }
        this.newDiscussions.push(this.newDiscussion);
      });
    }, 1000);
  }

  //Function that will take in a search string and then filter
  //the dicussions to show matching results
  setTable(){
    
    let input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("myInput");
    filter = input.value.toUpperCase();
    table = document.getElementById("myTable");
    tr = table.getElementsByTagName("tr");
    for (i = 0; i < tr.length; i++) {
      td = tr[i].getElementsByTagName("td")[0];
      if (td) {
        txtValue = td.textContent || td.innerText;
        if (txtValue.toUpperCase().indexOf(filter) > -1) {
          tr[i].style.display = "";
        } else {
          tr[i].style.display = "none";
        }
      }       
    }
    }

  //Function that will get a list of discussions for a movie
  //sorted in ascending order based on number of comments
  async sortDiscussionsByCommentsAsc() {
    setTimeout(() => {
      this._forum.sortDiscussionByCommentsAsc().subscribe(data => {
        console.log(data);
        this.sortDiscussions = data;
        this.newDiscussions = [];   
      });
    }, 1000);
  }

  //Function that will get a list of discussions for a movie
  //sorted in descending order based on number of comments
  async sortDiscussionsByCommentsDesc() {
    setTimeout(() => {
      this._forum.sortDiscussionByCommentsDesc().subscribe(data => {
        console.log(data);
        this.sortDiscussions = data;
        this.newDiscussions = [];
      });
    }, 1000);
  }

}
