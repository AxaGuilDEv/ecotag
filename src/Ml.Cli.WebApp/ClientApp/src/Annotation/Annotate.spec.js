﻿import React from "react";
import {fireEvent, render, waitFor} from "@testing-library/react";
import Annotate from "./Annotate";
import sleep from "../sleep";
import {BrowserRouter} from "react-router-dom";

const dataset = (`{"DatasetLocation": "C:\\\\location", "AnnotationType": "JsonEditor", "Configuration": \"[{\\\"name\\\": \\\"Recto\\\", \\\"id\\\": 0}, {\\\"name\\\": \\\"Verso\\\", \\\"id\\\": 1}]\", "Content": [{\"FileName\": \"{FileName}_pdf.json\",\"FileDirectory\": \"fileDirectoryValue\",\"ImageDirectory\": \"imageDirectoryValue\",\"Annotations\":{}}]}`);

const fetch = (status =200) => async (url, config) => {
    await sleep(1);
    console.log(url);
    return {
        status:status,
        json: async () => {
            await sleep(1);
            return ["C:\\\\imageLocation"];
        },
    };
};

describe("Check dataset handling", () => {
    test("Should insert dataset and select ocr and rotation annotation types", async () => {
        const mockedMonacoEditor = () => (<div>This is a mocked Monaco Editor</div>);
        const {container, asFragment, getAllByText, getByAltText} = render(<BrowserRouter><Annotate MonacoEditor={mockedMonacoEditor} fetchFunction={fetch(200)}/></BrowserRouter>);

        const blob = new Blob([dataset], {type: "text/plain;charset=utf-8"});
        const fileSource = new File([blob], "dataSourceFile.json", {type: "application/json"});
        const fileInput = container.querySelector("input[type='file']");
        fireEvent.change(fileInput, {target: {files: [fileSource]}});

        await waitFor(() => expect(container.querySelector('.table-result')).not.toBeNull());
        await waitFor(() => expect(getAllByText(/Visualising file: dataSourceFile.json/i)).not.toBeNull());
        await waitFor(() => expect(getByAltText(/file_image/i)).not.toBeNull());
        expect(asFragment()).toMatchSnapshot();
    });
});
