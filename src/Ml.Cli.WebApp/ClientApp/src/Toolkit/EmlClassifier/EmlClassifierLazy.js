﻿import React, {Suspense} from "react";

const EmlClassifier = React.lazy(() => import('./EmlClassifier'));

const EmlClassifierLazy = ({url, labels, onSubmit, expectedOutput, filename}) => {
    
    return <Suspense fallback={<div>Loading...</div>}>
        <EmlClassifier
            url={url}
            labels={labels}
            onSubmit={onSubmit}
            expectedOutput={expectedOutput}
            filename={filename}
        />
    </Suspense>
    
};

export default EmlClassifierLazy;
