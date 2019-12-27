import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-git-commits',
  templateUrl: './git-commits.component.html'
})
export class GitCommitsComponent {
  public commits: GitCommitsViewModel[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<GitCommitsViewModel[]>(baseUrl + 'api/GitCommits/GetCommits').subscribe(result => {
      debugger;
      this.commits = result;
    }, error => console.error(error));
  }
}

interface GitCommitsViewModel {
  committer: string;
  committedAt: string;
  message: string;
  htmlUrl: string;
}
