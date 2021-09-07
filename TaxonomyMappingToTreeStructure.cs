/* 
* This is an example to fetch multiple level taxonomy from metadata in a tree structure in below entity structure
*/

/* entity 
using System.Collections.Generic;

namespace DataEntities
{
    public class TermSetOrg
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PathOfTerm { get; set; }
        public List<TermSetOrg> ChildTermSetOrgs { get; set; }
      
    }
}

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.Practices.Unity;
using Microsoft.SharePoint.Client.Taxonomy;

namespace TaxonomyWork
{
        public async Task<TermSetOrg> getOrganisationTerms()
        {
            using (ClientContext clientContext = ClientContextHelper.GetClientContext())
            {

                var taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxonomySession.GetDefaultSiteCollectionTermStore();

                // Get the term group by Name
                TermGroup termGroup = termStore.Groups.GetByName("Site Collection - microsoft.sharepoint.com-teams-EnterpriseServicesBusinessRules");
                // Get the term set by Name
                TermSet termSet = termGroup.TermSets.GetByName("Organizations");
                clientContext.Load(termSet);
                clientContext.ExecuteQuery();

                var finalMappedTerms =  await GetTerms(clientContext, termSet.Terms, termSet.Name, termSet.Id.ToString(), null, null).ConfigureAwait(false);

                return finalMappedTerms;
            }
        }

        public async Task<TermSetOrg> GetTerms(ClientContext clientContext, TermCollection terms, string Name, string ID, string pathOfTerm , List<TermSetOrg> childTermsInNewEntity)
        {
            var ChildTerms = terms;
            clientContext.Load(ChildTerms);
            clientContext.ExecuteQuery();
            TermSetOrg mappedTerms = new TermSetOrg();
            mappedTerms.Id = ID;
            mappedTerms.Name = Name;
            mappedTerms.PathOfTerm = pathOfTerm;

            mappedTerms.ChildTermSetOrgs =new List<TermSetOrg>();
            // Process all child terms
            foreach (var ChildTerm in ChildTerms)
                {
                   mappedTerms.ChildTermSetOrgs.Add( await GetTerms(clientContext, ChildTerm.Terms, ChildTerm.Name, ChildTerm.Id.ToString(), ChildTerm.PathOfTerm, mappedTerms.ChildTermSetOrgs).ConfigureAwait(false));
                }
            return mappedTerms;  
        }
    }
}
