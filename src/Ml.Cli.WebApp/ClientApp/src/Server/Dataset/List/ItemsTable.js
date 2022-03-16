﻿import React from "react";
import Table, { Paging } from '@axa-fr/react-toolkit-table';
import Loader from '@axa-fr/react-toolkit-loader';
import HeaderColumnCell from "../../Project/List/ColumnHeader";
import Action from "@axa-fr/react-toolkit-action";
import {useHistory} from "react-router-dom";
import {formatTimestampToString} from "../../date";

const ItemsTable = ({items, filters, loaderMode, onChangePaging}) => {

    const history = useHistory();
    
    const editDatasetButton = id => {
        const path = `datasets/${id}`
        history.push(path);
    };
    
    return(
            <Loader mode={loaderMode}>
                <Table>
                    <Table.Header>
                        <Table.Tr>
                            <Table.Th>
                                <span className="af-table__th-content">Verrouiller</span>
                            </Table.Th>
                            <Table.Th>
                                <span className="af-table__th-content">Nom</span>
                            </Table.Th>
                            <Table.Th>
                                <span className="af-table__th-content">Classification</span>
                            </Table.Th>
                            <Table.Th>
                                <span className="af-table__th-content">Nombre de fichiers</span>
                            </Table.Th>
                            <Table.Th>
                                <span className="af-table__th-content">Type</span>
                            </Table.Th>
                            <Table.Th>
                                <span className="af-table__th-content">Date création</span>
                            </Table.Th>
                            <Table.Th><span className="af-table__th-content">Action</span></Table.Th>
                        </Table.Tr>
                    </Table.Header>
                    <Table.Body>
                        {items.map(
                            ({id, name, type, classification, numberFiles, createDate, isLocked}) => (
                                <Table.Tr key={id}>
                                    <Table.Td>
                                        <Action className={isLocked ? 'btn af-btn--circle af-btn--locked' : 'btn af-btn--circle'}
                                                id="lock" icon={isLocked ? "lock" : "unlock"}
                                                title={isLocked ? "vérouillée" : "dévérouillée"}
                                                onClick={() => {}} />
                                    </Table.Td>
                                    <Table.Td>
                                        {name}
                                    </Table.Td>
                                    <Table.Td>{classification}</Table.Td>
                                    <Table.Td>{numberFiles}</Table.Td>
                                    <Table.Td>{type}</Table.Td>
                                    <Table.Td>{formatTimestampToString(createDate )}</Table.Td>
                                    <Table.Td>
                                        <Action id="id" icon="edit" title="Editer" onClick={() => {editDatasetButton(id)}} />
                                    </Table.Td>
                                </Table.Tr>
                            )
                        )}
                    </Table.Body>
                </Table>
                <Paging
                    onChange={onChangePaging}
                    numberItems={filters.paging.numberItemsByPage}
                    numberPages={filters.paging.numberPages}
                    currentPage={filters.paging.currentPage}
                    id="home_paging"
                />
            </Loader>

    )
};

export default ItemsTable;