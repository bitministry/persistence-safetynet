// <copyright file="UnprocessedRequestsQueue.cs" company="BitMinistry">
// Copyright (c) 2022 All Rights Reserved
// Licensed under the Apache License, Version 2.0
// </copyright>
// <author>Andrew Rebane</author>
// <date>2022-7-26</date>
// <summary>A lightweight safety net for persistance layer</summary>


using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Turnit.GenericStore.WebApi.Common
{
    public class UnprocessedRequestsQueue
    {
        public static string BaseLocation;

        readonly DirectoryInfo _controllerDirectory;
        readonly HashSet<string> _iAmChecking = new HashSet<string>();
        public UnprocessedRequestsQueue(string controllerName )
        {
            _controllerDirectory = new DirectoryInfo($"{BaseLocation}/{ controllerName}");
            if (!_controllerDirectory.Exists)
                _controllerDirectory.Create();
        }

        public async Task<ActionResult> ExecuteRequest<TDto>(TDto currentInput, Func<TDto, Task<ActionResult>> action) {

            await Task.Run(() => CheckQueueForPreviousUnprocessedRequests(action));

            return await action.Invoke(currentInput);
        }

        public void CheckQueueForPreviousUnprocessedRequests<TDto>(Func<TDto, Task<ActionResult>> action)
        {
            if (_iAmChecking.Contains(typeof(TDto).Name)) return;
            _iAmChecking.Add(typeof(TDto).Name);

            object lockObj = new object();

            lock (lockObj) {
                var unprocessedRequests = getActionDirectory<TDto>()
                    .GetFiles()
                    .OrderBy(x => x.CreationTime)
                    .Select(file => new {
                        json = File.ReadAllText(file.FullName),
                        path = file.FullName
                    }).ToArray();

                foreach (var previousRequest in unprocessedRequests)
                {
                    File.Delete(previousRequest.path);

                    var request = JsonSerializer.Deserialize<UnprocessedRequest<TDto>>(previousRequest.json);
                    action.Invoke(request.Model);
                }
            }

            _iAmChecking.Remove(typeof(TDto).Name);

            
        }

        public void Enqueue<TDto>(TDto model, Exception ex)
        {
            var json = JsonSerializer.Serialize(new UnprocessedRequest<TDto>
            {
                Exception = ex.Message,
                Model = model
            });

            File.WriteAllText($"{getActionDirectory<TDto>().FullName}/{Guid.NewGuid()}.txt", json);
        }

        DirectoryInfo getActionDirectory<TDto>()
        {
            var actionDirectory = new DirectoryInfo(_controllerDirectory.FullName + "/" + typeof(TDto).Name);
            if (!actionDirectory.Exists) actionDirectory.Create();
            return actionDirectory;
        }

        public class UnprocessedRequest<TModelDto>
        {
            public string Exception { get; set; }
            public TModelDto Model { get; set; }

        }

    }

}