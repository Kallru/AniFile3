using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scriping
{
    public abstract class ScriperBase
    {
        public enum PageAsyncResult
        {
            None,
            Timeout,
        }

        // 기본 5초
        protected int _defaultTimeout = 10000;
        protected WebBrowser _browser;

        protected ScriperBase(WebBrowser browser)
        {
            _browser = browser;
        }

        protected async Task<PageAsyncResult> GetPageAsync(Action<HtmlDocument> requestWork, Func<HtmlDocument, bool> finishingCondition = null)
        {
            var compeltionSource = new TaskCompletionSource<PageAsyncResult>();

            void OnPageCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
            {
                var browser = sender as WebBrowser;
                if (browser != null)
                {
                    HtmlDocument document = browser.Document;

                    // 이번 로딩을 끝낼지 말지 체크한다
                    if (finishingCondition == null ||
                        finishingCondition(document))
                    {
                        compeltionSource.SetResult(PageAsyncResult.None);
                        _browser.DocumentCompleted -= OnPageCompleted;
                    }
                }

                Console.WriteLine("OnPageCompleted");
            }

            _browser.DocumentCompleted += OnPageCompleted;

            if (requestWork != null)
                requestWork(_browser.Document);

            // 타임아웃 처리
            var task = compeltionSource.Task;
            if (await Task.WhenAny(task, Task.Delay(_defaultTimeout)) == task)
            {
                // Done
                return task.Result;
            }
            else
            {
                // timeout
                return PageAsyncResult.Timeout;
            }
        }

        protected HtmlElement FindElement(string tag, string className = "", string typeName = "", HtmlElement parents = null)
        {
            // 비교 조건 넣기
            List<Func<HtmlElement, bool>> conditions = new List<Func<HtmlElement, bool>>();
            if (string.IsNullOrEmpty(className) == false)
                conditions.Add((element => element.GetAttribute("className") == className));

            if (string.IsNullOrEmpty(typeName) == false)
                conditions.Add((element => element.GetAttribute("type") == typeName));

            HtmlElementCollection collection;
            if (parents == null)
                collection = _browser.Document.GetElementsByTagName(tag);
            else
                collection = parents.GetElementsByTagName(tag);

            foreach (HtmlElement element in collection)
            {
                if (conditions.TrueForAll((condition) => condition(element)))
                {
                    return element;
                }
            }
            return null;
        }
    }
}
