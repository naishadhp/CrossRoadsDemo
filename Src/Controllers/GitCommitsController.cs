using CrossroadsDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrossroadsDemo.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    public class GitCommitsController : Controller
    {
        /// <summary>
        /// The _config
        /// </summary>
        private readonly IConfiguration _config;
        /// <summary>
        /// Gets or sets the name of the _git user.
        /// </summary>
        /// <value>
        /// The name of the _git user.
        /// </value>
        private string _gitUserName { get; set; }
        /// <summary>
        /// Gets or sets the _git password.
        /// </summary>
        /// <value>
        /// The _git password.
        /// </value>
        private string _gitPassword { get; set; }
        /// <summary>
        /// Gets or sets the _git repository.
        /// </summary>
        /// <value>
        /// The _git repository.
        /// </value>
        private long _gitRepository { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitCommitsController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public GitCommitsController(IConfiguration configuration)
        {
            _config = configuration;
            _gitUserName = _config.GetValue<string>("GitSettings:GitUsername");
            _gitPassword = _config.GetValue<string>("GitSettings:GitPassword");
            _gitRepository = _config.GetValue<long>("GitSettings:GitRepositoryId");
        }

        /// <summary>
        /// Gets the commits.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">Not able to get commits due to exception:  + ex.Message</exception>
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
