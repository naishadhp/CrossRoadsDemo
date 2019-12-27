using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossroadsDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Octokit;

namespace CrossroadsDemo.Controllers
{
    [Route("api/[controller]")]
    public class GitCommitsController : Controller
    {
        private readonly IConfiguration _config;
        private string _gitUserName { get; set; }
        private string _gitPassword { get; set; }
        private long _gitRepository { get; set; }

        public GitCommitsController(IConfiguration configuration)
        {
            _config = configuration;
            _gitUserName = _config.GetValue<string>("GitSettings:GitUsername");
            _gitPassword = _config.GetValue<string>("GitSettings:GitPassword");
            _gitRepository = _config.GetValue<long>("GitSettings:GitRepositoryId");
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<GitCommitsViewModel>> GetCommits()
        {
            try
            {
                GitHubClient client = new GitHubClient(new ProductHeaderValue("SampleApp"), new Uri("https://api.github.com/"));
                client.Credentials = new Credentials(_gitUserName, _gitPassword);

                // Current Repository => repoId = 230391516 
                var repositoryDetail = await client.Repository.Get(_gitRepository);
                if (repositoryDetail != null)
                {
                    var commits = await client.Repository.Commit.GetAll(repositoryDetail.Id);
                    if (commits != null && commits.Count > 0)
                    {
                        return commits.Select(x => new GitCommitsViewModel()
                        {
                            Committer = x.Commit.Committer.Name,
                            CommittedAt = x.Commit.Committer.Date.ToLocalTime().ToString("MMM dd, yyyy hh:mm tt"),
                            Message = x.Commit.Message,
                            HtmlUrl = x.HtmlUrl
                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // Either the accessToken is null or it's invalid. This redirects
                // to the GitHub OAuth login page. That page will redirect back to the
                // Authorize action.
                throw new Exception("Not able to get commits due to exception: " + ex.Message);
            }
            return new List<GitCommitsViewModel>();
        }
    }
}
