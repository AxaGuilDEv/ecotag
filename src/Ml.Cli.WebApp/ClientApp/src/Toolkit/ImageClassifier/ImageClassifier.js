﻿import React from "react";
import Button from "@axa-fr/react-toolkit-button";
import stringToRGB from "./color";
import '../BoundingBox/Labels.scss';
import './ImageClassifier.scss';
import '@axa-fr/react-toolkit-button/src/button.scss'
import classNames from "classnames";
import { GlobalHotKeys } from 'react-hotkeys';

const defaultClassName = 'image-classifier';
const defaultClassNameButtonsContainer = 'image-classifier__buttons-container';
const defaultClassNameButtonContainer = 'image-classifier__button-container';

const generateKeyMap = (length) => {
    let result = {};
    for(let i = 1; i <= length; i ++){
        result[`${i.toString(16)}`] = `${i.toString(16)}`;
    }
    return result;
};

const ImageClassifier = ({url, labels, onSubmit, state}) => {
    const className = classNames(defaultClassName, {
        [`${defaultClassName}--inline-mode`]: state.inlineMode,
    });
    const classNameButtonsContainer = classNames(defaultClassNameButtonsContainer, {
        [`${defaultClassNameButtonsContainer}--inline-mode`]: state.inlineMode,
    });
    const classNameButtonContainer = classNames(defaultClassNameButtonContainer, {
        [`${defaultClassNameButtonContainer}--inline-mode`]: state.inlineMode,
    });
    const coloredLabels = labels.map((label) => {
        return {
            "name": label.name,
            "color": `#${stringToRGB(label.name)}`
        };
    });

    const generateHandler = () => {
        let result = {};
        for(let i = 0; i <= coloredLabels.length; i++){
            result[`${i.toString(16)}`] = () => console.log(coloredLabels[i - 1].name);
        }
        return result;
    }
    
    const keyMap = generateKeyMap(coloredLabels.length);
    
    const handlers = generateHandler();
    
    return(
        <>
            <GlobalHotKeys allowChanges={true} keyMap={keyMap} handlers={handlers}>
                <div className={className}>
                    <div className={classNameButtonsContainer}>
                        {coloredLabels.map((label, index) => {
                            return(
                                <div key={index} className={classNameButtonContainer}>
                                    <Button onClick={() => onSubmit(label.name)} style={{backgroundColor: label.color, boxShadow: "none"}}>{label.name}</Button>
                                </div>
                            );
                        })}
                    </div>
                    <div className="image-classifier__image-container">
                        <img
                            src={url}
                            id="currentImage"
                            alt="Classifier image"
                            style={{
                                width: `${state.widthImage}%`,
                                transform: `rotate(${state.rotate}deg)`,
                                margin: `${state.initialRotate ? '' : state.marginRotate}`,
                            }}
                        />
                    </div>
                </div>
            </GlobalHotKeys>
        </>
    );
};

export default ImageClassifier;