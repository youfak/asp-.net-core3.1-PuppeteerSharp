using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharpTest
{
    public sealed class PuppeteerSharpUtil
    {

        private static Browser _browser { get; set; }

        /// <summary>
        /// 异步线程锁(让浏览器只初始化一次)
        /// </summary>
        private static SemaphoreSlim slimLock = new SemaphoreSlim(1, 1);
        /// <summary>
        /// 初始化浏览
        /// </summary>
        /// <returns></returns>
        private async static Task InItBrowser()
        {
            try
            {
                await slimLock.WaitAsync();
                if (_browser == null)
                {
                    //不存在则下载浏览器
                    var result = await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
                    _browser = await Puppeteer.LaunchAsync(new LaunchOptions
                    {
                        Headless = true,
                        Args = new string[] {
                            "--disable-infobars",//隐藏 自动化标题
                            "--no-sandbox", // 沙盒模式
                        },
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                slimLock.Release();
            }
        }

        /// <summary>
        /// 异步线程锁(锁定每次只处理10个任务)
        /// </summary>
        private static readonly SemaphoreSlim _mutex = new SemaphoreSlim(10);

        /// <summary>
        /// 异步截图
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        /// <param name="isTagName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static async Task ScreenshotAsync(string url, string savePath,int width = 1000, int height = 1000)
        {
            await _mutex.WaitAsync();
            if (_browser == null) await InItBrowser();
            try
            {
                await Task.Run(async () =>
                {
                    using Page page = await _browser.NewPageAsync();
                    await page.SetViewportAsync(new ViewPortOptions
                    {
                        Width = width,
                        Height = height
                    });
                    WaitUntilNavigation[] wn = new WaitUntilNavigation[50];
                    wn[0] = WaitUntilNavigation.DOMContentLoaded;
                    wn[1] = WaitUntilNavigation.Load;
                    await page.GoToAsync(url, new NavigationOptions() { WaitUntil = wn });
                    try
                    {
                        await page.ScreenshotAsync(savePath, new ScreenshotOptions() { Type = ScreenshotType.Png });
                    }
                    catch (Exception ex)
                    {
                        NLogHelp.ErrorLog("截图出错", ex);
                    }
                });

            }
            catch (Exception ex)
            {
                GC.Collect();
                NLogHelp.ErrorLog("截图----出错:", ex);
                throw ex;
            }
            finally
            {
                _mutex.Release();
                GC.Collect();
            }
        }

        /// <summary>
        /// 异步截图
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        /// <param name="isTagName"></param>
        /// <param name="action"></param>
        /// <param name="par1"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static async Task ScreenshotAsync<T1>(string url, string savePath, string isTagName, Action<T1> action, T1 par1, int width = 1000, int height = 1000)
        {
            try
            {
                await _mutex.WaitAsync();
                if (_browser == null) await InItBrowser();
                using Page page = await _browser.NewPageAsync();
                page.Load += async (sender, e) =>
                {
                    try
                    {
                        var _page = (Page)sender;
                        if (!string.IsNullOrEmpty(isTagName))
                        {
                            var tag = await page.QuerySelectorAsync(isTagName);
                            if (tag != null)
                            {
                                await page.ScreenshotAsync(savePath, new ScreenshotOptions() { Type = ScreenshotType.Png });
                                action?.Invoke(par1);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        NLogHelp.ErrorLog("截图----Load出错:", ex);
                    }
                };
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = width,
                    Height = height
                });
                await page.GoToAsync(url);
            }
            catch (Exception ex)
            {
                NLogHelp.ErrorLog("截图----出错:", ex);
                GC.Collect();
                throw ex;
            }
            finally
            {
                _mutex.Release();
                GC.Collect();
            }
        }

        /// <summary>
        /// 异步截图
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        /// <param name="isTagName"></param>
        /// <param name="action"></param>
        /// <param name="par1"></param>
        /// <param name="par2"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static async Task ScreenshotAsync<T1, T2>(string url, string savePath, string isTagName, Action<T1, T2> action, T1 par1, T2 par2, int width = 1000, int height = 1000)
        {
            try
            {
                await _mutex.WaitAsync();
                if (_browser == null) await InItBrowser();
                using Page page = await _browser.NewPageAsync();
                page.Load += async (sender, e) =>
                {
                    try
                    {
                        var _page = (Page)sender;
                        if (!string.IsNullOrEmpty(isTagName))
                        {
                            var tag = await page.QuerySelectorAsync(isTagName);
                            if (tag != null)
                            {
                                await page.ScreenshotAsync(savePath, new ScreenshotOptions() { Type = ScreenshotType.Png });
                                action?.Invoke(par1, par2);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        NLogHelp.ErrorLog("截图----Load出错:", ex);
                    }
                };
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = width,
                    Height = height
                });
                await page.GoToAsync(url);
            }
            catch (Exception ex)
            {
                NLogHelp.ErrorLog("截图----出错:", ex);
                GC.Collect();
                throw ex;
            }
            finally
            {
                _mutex.Release();
                GC.Collect();
            }
        }

        /// <summary>
        /// 异步截图
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        /// <param name="isTagName"></param>
        /// <param name="action"></param>
        /// <param name="par1"></param>
        /// <param name="par2"></param>
        /// <param name="par3"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static async Task ScreenshotAsync<T1, T2, T3>(string url, string savePath, string isTagName, Action<T1, T2, T3> action, T1 par1, T2 par2, T3 par3, int width = 1080, int height = 1920)
        {
            await _mutex.WaitAsync();
            if (_browser == null) await InItBrowser();
            try
            {
                await Task.Run(async () =>
                {
                    using Page page = await _browser.NewPageAsync();
                    await page.SetViewportAsync(new ViewPortOptions
                    {
                        Width = width,
                        Height = height
                    });
                    WaitUntilNavigation[] wn = new WaitUntilNavigation[50];
                    wn[0] = WaitUntilNavigation.DOMContentLoaded;
                    wn[1] = WaitUntilNavigation.Load;
                    await page.GoToAsync(url, new NavigationOptions() { WaitUntil = wn });
                    try
                    {
                        //await page.PdfAsync(savePath);
                        await page.ScreenshotAsync(savePath, new ScreenshotOptions() { Type = ScreenshotType.Png });
                        action?.Invoke(par1, par2, par3);
                    }
                    catch (Exception ex)
                    {
                        NLogHelp.ErrorLog("截图出错", ex);
                    }
                });

            }
            catch (Exception ex)
            {
                GC.Collect();
                NLogHelp.ErrorLog("截图----出错:", ex);
                throw ex;
            }
            finally
            {
                _mutex.Release();
                GC.Collect();
            }
        }

        /// <summary>
        /// 异步截图
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        /// <param name="isTagName"></param>
        /// <param name="action"></param>
        /// <param name="par1"></param>
        /// <param name="par2"></param>
        /// <param name="par3"></param>
        /// <param name="par4"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static async Task ScreenshotAsync<T1, T2, T3, T4>(string url, string savePath, string isTagName, Action<T1, T2, T3, T4> action, T1 par1, T2 par2, T3 par3, T4 par4, int width = 1000, int height = 1000)
        {
            NLogHelp.InfoLog(url + "截图进来时间----:" + par4 + "  -----------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            await _mutex.WaitAsync();
            if (_browser == null) await InItBrowser();
            try
            {
                //Action<T4> action1 = async (time) =>
                //{
                //    using Page page = await _browser.NewPageAsync();
                //    await page.SetViewportAsync(new ViewPortOptions
                //    {
                //        Width = width,
                //        Height = height
                //    });
                //    WaitUntilNavigation[] wn = new WaitUntilNavigation[50];
                //    wn[0] = WaitUntilNavigation.DOMContentLoaded;
                //    wn[1] = WaitUntilNavigation.Load;
                //    await page.GoToAsync(url, new NavigationOptions() { WaitUntil = wn });
                //    try
                //    {
                //        //await page.PdfAsync(savePath);
                //        NLogHelp.InfoLog(url + "截图开始时间----:" + time + "  -----------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //        await page.ScreenshotAsync(savePath, new ScreenshotOptions() { Type = ScreenshotType.Png });
                //        NLogHelp.InfoLog(url + "截图结束时间----:" + time + "  -----------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //        action?.Invoke(par1, par2, par3, time);
                //    }
                //    catch (Exception ex)
                //    {
                //        NLogHelp.ErrorLog("截图出错", ex);
                //    }

                //};
                //action1(par4);
                await Task.Run(async () =>
                {
                    T4 time = par4;
                    using Page page = await _browser.NewPageAsync();
                    await page.SetViewportAsync(new ViewPortOptions
                    {
                        Width = width,
                        Height = height
                    });
                    WaitUntilNavigation[] wn = new WaitUntilNavigation[50];
                    wn[0] = WaitUntilNavigation.DOMContentLoaded;
                    wn[1] = WaitUntilNavigation.Load;
                    await page.GoToAsync(url, new NavigationOptions() { WaitUntil = wn });
                    try
                    {
                        //await page.PdfAsync(savePath);
                        NLogHelp.InfoLog(url + "截图开始时间----:" + time + "  -----------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        await page.ScreenshotAsync(savePath, new ScreenshotOptions() { Type = ScreenshotType.Png });
                        NLogHelp.InfoLog(url + "截图结束时间----:" + time + "  -----------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        action?.Invoke(par1, par2, par3, time);
                    }
                    catch (Exception ex)
                    {
                        NLogHelp.ErrorLog("截图出错", ex);
                    }
                });

            }
            catch (Exception ex)
            {
                GC.Collect();
                NLogHelp.ErrorLog("截图----出错:", ex);
                throw ex;
            }
            finally
            {
                _mutex.Release();
                GC.Collect();
            }
        }

    }

}
