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
  // searchDiscussions = [];
  topics: any;
  
  newDiscussions = [];
  sortDiscussions: any;
  comments: any;
  commentCount: any;
  movieID:string = "";
  discussionID: any;
  DisplayList: boolean = true;

  pageNum: number = 1;
  sortingOrder: string = "recent"   //Default sorting order will be based on total num of comments 
  numOfDiscussion: number;

  constructor(private _forum: ForumService, private router:  ActivatedRoute) { }

  ngOnInit(): void {
    
  
    this.movieID =  this.router.snapshot.params.id;
    console.log("Discussion List:" + this.movieID);
    this.getDiscussions();
    this._forum.getDiscussion(this.movieID).subscribe(data =>{ 
      this.discussions = data;
      this.numOfDiscussion = this.discussions.length
      this.discussions = []})
    this._forum.getTopics().subscribe(data => {
      console.log(data);
      this.topics = data;
    });
  }

  //Function that will get a list of discussions associated with the
  //snapshot movie id
  async getDiscussions() {
    this.discussions = [];
    setTimeout(() => {
      this._forum.getDiscussionPage(this.movieID, this.pageNum, this.sortingOrder).subscribe(data => {
        console.log(data);
        
        this.discussions = data;
        console.log(this.discussions);   
      });
    }, 1000);
  }

  //get next discussion page
  onNext(){
    //this.discussions = [];
    this.pageNum++;
    this.getDiscussions();
  }
  //get previous duscussuin page
  onPrev(){
    //this.discussions = [];
    this.pageNum--;
    this.getDiscussions();
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
    this.sortingOrder = "commentsA";
    this.getDiscussions()
    // setTimeout(() => {
    //   this._forum.sortDiscussionByCommentsAsc().subscribe(data => {
    //     console.log(data);
    //     this.sortDiscussions = data;
    //     this.discussions = [];   
    //   });
    // }, 1000);
  }

  //Function that will get a list of discussions for a movie
  //sorted in descending order based on number of comments
  async sortDiscussionsByCommentsDesc() {
    this.sortingOrder = "commentsD";
    this.getDiscussions();
    // setTimeout(() => {
    //   this._forum.sortDiscussionByCommentsDesc().subscribe(data => {
    //     console.log(data);
    //     this.sortDiscussions = data;
    //     this.discussions = [];
    //   });
    // }, 1000);
  }

  sortByCreationA(){
    this.sortingOrder = "timeA";
    this.getDiscussions();
  }
  sortByCreationB(){
    this.sortingOrder = "timeD";
    this.getDiscussions();
  }
  sortByRecent(){
    this.sortingOrder = "recent";
    this.getDiscussions();
  }
  sortByLike(){
    this.sortingOrder = "like";
    this.getDiscussions();
  }

  async filterByTopic(topicid: string)
  {
    console.log(topicid)
    setTimeout(() => {
      this._forum.filterDiscussionByTopic(topicid).subscribe(data => {
        console.log(data);
        this.discussions = data;
      })
    })
  }

}
